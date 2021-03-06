import * as React from 'react';
import { Row, Col, Card, Image } from 'react-bootstrap';
import './Legend.scss';

export const Legend = () => {
  const keys = React.useMemo(() => {
    return [
      {
        pin: require('assets/images/marker-icon-2x-green.png'),
        label: 'Parcel',
      },
      {
        pin: require('assets/images/marker-icon-2x-blue.png'),
        label: 'Building',
      },
      {
        pin: require('assets/images/marker-icon-2x-red.png'),
        label: 'Enhanced Referral Process',
      },
      {
        pin: require('assets/images/marker-icon-2x-violet.png'),
        label: 'Surplus Properties Program',
      },
    ];
  }, []);

  return (
    <Card className="legend-control">
      <Card.Header>Marker Legend</Card.Header>

      <Card.Body>
        {keys.map((item, index) => {
          return (
            <Row key={index}>
              <Col xs={2}>
                <Image height={25} src={item.pin} />
              </Col>
              <Col className="label">{item.label}</Col>
            </Row>
          );
        })}
      </Card.Body>
    </Card>
  );
};
